#pragma once

#if ARDUINO

#ifndef _MPU6050_H_
#include <MPU6050.h>
#endif

class Calibration {
    public:
        Calibration(MPU6050 *mpu);
        int* Calibrate();
        void SetOffsets(int TheOffsets[6]);

    private:
        MPU6050 *accelgyro;
        int LowValue[6];
        int HighValue[6];
        int Smoothed[6];
        int LowOffset[6];
        int HighOffset[6];
        int Target[6];
        int N;
        void SetAveraging(int NewN);
        void PullBracketsOut();
        void PullBracketsIn();
        void ShowProgress();
        void GetSmoothed();
};
#endif