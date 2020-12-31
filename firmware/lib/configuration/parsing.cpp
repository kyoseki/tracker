#include <stdint.h>
#include <cstring>
#include <stdlib.h>
#include "parsing.h"

#define MAC_ADDRESS_BYTES 6
#define IP_ADDRESS_BYTES 4
#define SERIAL_MSG_LENGTH 32

#ifdef ARDUINO
#include <Arduino.h>

#define CONFIG_PIN 16
int checkBoot() {
    pinMode(CONFIG_PIN, INPUT);

    return digitalRead(CONFIG_PIN);
}

char* listen() {
    static int i = 0;
    static char received[SERIAL_MSG_LENGTH + 1];
    char endMarker = '\n';
    char rc;

    while(Serial.available() > 0) {
        rc = Serial.read();

        if (rc == endMarker) {
            i = 0;
            return received;
        }

        if (i >= SERIAL_MSG_LENGTH - 1) {
            i = 0;
            return nullptr;
        }

        received[i] = rc;
        i++;
    }

    return nullptr;
}
#endif

int parseReceiver(char* message, receiverConfig* config) {
    char* command = strtok(message, " ");
    char* arg = strtok(NULL, " ");

    /*if (strcmp(command, "ssid") == 0) {
        strcpy(config->ssid, arg);
    } else if (strcmp(command, "pass") == 0) {
        strcpy(config->password, arg);
    } else*/ if (strcmp(command, "id") == 0) {
        config->id = atoi(arg);
    } /*else if (strcmp(command, "ip") == 0) {
        uint8_t output[4];
        int result = parseIpAddress(arg, output);

        if (result) {
            memcpy(config->ip, output, IP_ADDRESS_BYTES);
        } else {
            return 0;
        }
    }*/

    return 1;
}

int parseTransmitter(char* message, transmitterConfig* config) {
    char* command = strtok(message, " ");
    char* arg = strtok(NULL, " ");

    if (strcmp(command, "id") == 0) {
        config->id = atoi(arg);
    } else if (strcmp(command, "mac") == 0) {
        uint8_t output[6];
        int result = parseMacAddress(arg, output);

        if (result) {
            memcpy(config->mac, output, MAC_ADDRESS_BYTES);
        } else {
            return 0;
        }
    }

    return 1;
}

int parseIpAddress(char* input, uint8_t* output) {
    int segment = 0;
    char* split = strtok(input, ".");
    while (split != NULL) {
        if (strlen(split) > 3) {
            return 0;
        }

        output[segment] = atoi(split);
        split = strtok(NULL, ".");
        segment++;
    }

    if (segment != IP_ADDRESS_BYTES) {
        return 0;
    }

    return 1;
}

// Parse a MAC address from a char array string into an output uint8_t array.
int parseMacAddress(char* input, uint8_t* output) {
    if (strlen(input) != MAC_ADDRESS_BYTES * 2 + MAC_ADDRESS_BYTES - 1) {
        return 0;
    }

    int segment = 0;
    char* split = strtok(input, ":");
    while (split != NULL) {
        if (strlen(split) != 2) {
            return 0;
        }

        output[segment] = (uint8_t)strtol(split, nullptr, 16);
        split = strtok(NULL, ":");
        segment++;
    }

    if (segment != MAC_ADDRESS_BYTES) {
        return 0;
    }

    return 1;
}