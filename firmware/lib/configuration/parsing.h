#pragma once

#include <stdint.h>

typedef struct receiverConfig {
    int id;
    // char ssid[24];
    // char password[24];
    // uint8_t ip[4];
} receiverConfig;

typedef struct transmitterConfig {
    int id;
    uint8_t mac[6];
    int calibration[6];
} transmitterConfig;

typedef struct dataPacket {
    uint8_t id;
    uint8_t x;
    uint8_t y;
    uint8_t z;
    uint8_t w;
} dataPacket;

#ifdef ARDUINO
int checkBoot();
char* listen();
#endif

int parseReceiver(char* message, receiverConfig* config);

int parseTransmitter(char* message, transmitterConfig* config);

uint8_t encodeFloat(float value);

int parseIpAddress(char* input, uint8_t* output);

int parseMacAddress(char* input, uint8_t* output);
