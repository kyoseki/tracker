#include <Arduino.h>
#include <EEPROM.h>
#include "parsing.h"
#include <ESP8266WiFi.h>
#include <espnow.h>

receiverConfig config;

dataPacket data;

void dataCallback(uint8_t* mac, uint8_t *incomingData, uint8_t len) {
    memcpy(&data, incomingData, sizeof(data));

    Serial.print(config.id);
    Serial.print(" ");
    Serial.print(data.id);
    Serial.print(" ");
    Serial.print(data.w);
    Serial.print(" ");
    Serial.print(data.x);
    Serial.print(" ");
    Serial.print(data.y);
    Serial.print(" ");
    Serial.println(data.z);
}

void setup() {
    Serial.begin(115200);
    Serial.println();

    EEPROM.begin(sizeof(config));
    EEPROM.get(0, config);

    if (checkBoot()) {
        Serial.println("readout");
        Serial.print("id ");
        Serial.println(config.id);

        while (true) {
            char* command = listen();

            if (command) {
                if (strcmp(command, "commit") == 0) {
                    Serial.println("Committing to EEPROM...");
                    EEPROM.put(0, config);
                    EEPROM.commit();
                } else {
                    parseReceiver(command, &config);
                }
            }
        }
    }

    WiFi.mode(WIFI_STA);

    Serial.print("MAC Address: ");
    Serial.println(WiFi.macAddress());

    if (esp_now_init() != 0) {
        Serial.println("ESP-NOW failed to initialize");
        return;
    }

    esp_now_set_self_role(ESP_NOW_ROLE_SLAVE);

    esp_now_register_recv_cb(dataCallback);
}

void loop() {

}