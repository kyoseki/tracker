#include <Arduino.h>
#include <EEPROM.h>
#include "parsing.h"
#include <ESP8266WiFi.h>
#include <espnow.h>

#include <I2Cdev.h>
#include <MPU6050_6Axis_MotionApps20.h>
#include <Wire.h>

#include "calibrate.h"

#ifdef USE_INTERRUPT
#define INTERRUPT_PIN 14

uint8_t mpuIntStatus;
uint16_t fifoCount;
uint16_t packetSize;

volatile bool mpuInterrupt = false;
void ICACHE_RAM_ATTR dmpDataReady() {
    mpuInterrupt = true;
}
#endif

transmitterConfig config;

dataPacket data;

MPU6050 mpu;

uint8_t fifoBuffer[64];

Quaternion q;

bool dmpReady = false;

void setup() {
    Serial.begin(115200);
    Serial.println();

    EEPROM.begin(sizeof(config));
    EEPROM.get(0, config);

    Wire.begin();
    Wire.setClock(400000);

    Serial.println("Initializing MPU...");
    mpu.initialize();

    Serial.print("Testing MPU connection... ");
    if (mpu.testConnection()) {
        Serial.println("OK");
    } else {
        Serial.println("Failed!");
    }

    Calibration calib(&mpu);

    if (checkBoot()) {
        Serial.println("readout");
        Serial.print("id ");
        Serial.println(config.id);

        Serial.print("mac ");
        for (int i = 0; i < 6; i++) {
            Serial.print(config.mac[i]);

            if (i != 5) {
                Serial.print(" ");
            }
        }
        Serial.println();

        while (true) {
            char* command = listen();

            if (command) {
                if (strcmp(command, "commit") == 0) {
                    Serial.println("Committing to EEPROM...");
                    EEPROM.put(0, config);
                    EEPROM.commit();
                } else if (strcmp(command, "calibrate") == 0) {
                    int *result = calib.Calibrate();
                    memcpy(config.calibration, result, sizeof(result[0]) * 6);

                    Serial.println("Writing offsets to EEPROM...");
                    for (int i = 0; i < 6; i++) {
                        Serial.print(result[i]);
                        Serial.print(" ");
                    }
                    Serial.println();

                    EEPROM.put(0, config);
                    EEPROM.commit();
                } else {
                    parseTransmitter(command, &config);
                }
            }
        }
    }

    data.id = config.id;

    #ifdef USE_INTERRUPT
    pinMode(INTERRUPT_PIN, INPUT);
    #endif

    Serial.println("DMP programming will begin shortly...");
    delay(1000);

    uint8_t devStatus = mpu.dmpInitialize();

    if (devStatus == 0) {
        calib.SetOffsets(config.calibration);
        mpu.PrintActiveOffsets();

        Serial.println("Enabling DMP...");
        mpu.setDMPEnabled(true);

        #ifdef USE_INTERRUPT
        Serial.print("Receiving interrupts from pin ");
        Serial.println(digitalPinToInterrupt(INTERRUPT_PIN));
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();
        #endif

        dmpReady = true;

        #ifdef USE_INTERRUPT
        packetSize = mpu.dmpGetFIFOPacketSize();
        #endif
    }

    WiFi.mode(WIFI_STA);

    if (esp_now_init() != 0) {
        Serial.println("ESP-NOW failed to initialize");
        return;
    }

    esp_now_set_self_role(ESP_NOW_ROLE_CONTROLLER);
    esp_now_add_peer(config.mac, ESP_NOW_ROLE_SLAVE, 1, NULL, 0);
}

void loop() {
    if (!dmpReady) return;

    #ifdef USE_INTERRUPT
    if (!mpuInterrupt && fifoCount < packetSize) return;

    mpuInterrupt = false;
    mpuIntStatus = mpu.getIntStatus();

    fifoCount = mpu.getFIFOCount();

    if ((mpuIntStatus & 0x10) || fifoCount == 1024) {
        mpu.resetFIFO();
        Serial.println("FIFO overflow!");
    } else if (mpuIntStatus & 0x02) {
        while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

        mpu.getFIFOBytes(fifoBuffer, packetSize);

        fifoCount -= packetSize;

        mpu.dmpGetQuaternion(&q, fifoBuffer);
    }
    #else
    if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
        mpu.dmpGetQuaternion(&q, fifoBuffer);
    }
    #endif

    data.x = encodeFloat(q.x);
    data.y = encodeFloat(q.y);
    data.z = encodeFloat(q.z);
    data.w = encodeFloat(q.w);

    esp_now_send(NULL, (uint8_t*)&data, sizeof(data));
}