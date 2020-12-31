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
} transmitterConfig;

typedef struct dataPacket {
    int id;
    float x;
    float y;
    float z;
    float w;
} dataPacket;

#ifdef ARDUINO
int checkBoot();
char* listen();
#endif

int parseReceiver(char* message, receiverConfig* config);

int parseTransmitter(char* message, transmitterConfig* config);

int parseIpAddress(char* input, uint8_t* output);

int parseMacAddress(char* input, uint8_t* output);
