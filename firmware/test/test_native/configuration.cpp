#include <unity.h>
#include "parsing.h"
#include <stdio.h>
#include <string.h>

void test_parse_receiver(void) {
    receiverConfig* config = new receiverConfig;
    char message[32] = "ssid abc123";

    /*
    // Parsing basic string (ssid) should work
    parseReceiver(message, config);

    TEST_ASSERT_EQUAL(0, strcmp("abc123", config->ssid));
    */

    // Parsing int (id) should work
    strcpy(message, "id 12");

    parseReceiver(message, config);

    TEST_ASSERT_EQUAL(12, config->id);

    /*
    // Parsing IP address should work
    strcpy(message, "ip 192.168.1.1");

    parseReceiver(message, config);

    uint8_t correct[4] = { 192, 168, 1, 1 };

    for (int i = 0; i < 4; i++) {
        TEST_ASSERT_EQUAL(correct[i], config->ip[i]);
    }
    */
}

void test_parse_ip_address(void) {
    char input[32] = "192.168.1.1";
    uint8_t correct[4] = { 192, 168, 1, 1 };
    uint8_t output[4];

    // Valid address should succeed
    int result = parseIpAddress(input, output);

    TEST_ASSERT_EQUAL(1, result);

    for (int i = 0; i < 4; i++) {
        TEST_ASSERT_EQUAL(correct[i], output[i]);
    }

    // Address with segment >3 in length should fail
    strcpy(input, "1921.168.1.1");

    result = parseIpAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);

    // Address with >4 bytes should fail
    strcpy(input, "192.168.1.1.1");

    result = parseIpAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);

    // Address with <4 bytes should fail
    strcpy(input, "192.168.1");

    result = parseIpAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);
}

void test_parse_mac_address(void) {
    char input[32] = "A4:CF:12:c7:9c:77";
    uint8_t correct[6] = { 0xA4, 0xCF, 0x12, 0xC7, 0x9C, 0x77 };
    uint8_t output[6];

    // Valid address should succeed
    int result = parseMacAddress(input, output);

    TEST_ASSERT_EQUAL(1, result);

    for (int i = 0; i < 6; i++) {
        TEST_ASSERT_EQUAL(correct[i], output[i]);
    }

    // Address with segment >2 in length should fail
    strcpy(input, "A4A:FF:FF:FF:FF:FF");

    result = parseMacAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);

    // Address with <6 segments should fail
    strcpy(input, "FF:FF:FF");

    result = parseMacAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);

    // Address with >6 segments should fail
    strcpy(input, "FF:FF:FF:FF:FF:FF:FF");

    result = parseMacAddress(input, output);

    TEST_ASSERT_EQUAL(0, result);
}

int main() {
    UNITY_BEGIN();
    RUN_TEST(test_parse_ip_address);
    RUN_TEST(test_parse_mac_address);
    RUN_TEST(test_parse_receiver);
    UNITY_END();
}