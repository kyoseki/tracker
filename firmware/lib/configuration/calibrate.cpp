#if ARDUINO
// MPU6050 offset-finder, based on Jeff Rowberg's MPU6050_RAW
// 2016-10-19 by Robert R. Fenichel (bob@fenichel.net)
// Modified for kyoseki - original at https://github.com/jrowberg/i2cdevlib/blob/master/Arduino/MPU6050/examples/IMU_Zero/IMU_Zero.ino

/* ============================================
I2Cdev device library code is placed under the MIT license
Copyright (c) 2011 Jeff Rowberg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

  If an MPU6050
      * is an ideal member of its tribe,
      * is properly warmed up,
      * is at rest in a neutral position,
      * is in a location where the pull of gravity is exactly 1g, and
      * has been loaded with the best possible offsets,
then it will report 0 for all accelerations and displacements, except for
Z acceleration, for which it will report 16384 (that is, 2^14).  Your device
probably won't do quite this well, but good offsets will all get the baseline
outputs close to these target values.

  Put the MPU6050 on a flat and horizontal surface, and leave it operating for
5-10 minutes so its temperature gets stabilized.

  Run this program.  A "----- done -----" line will indicate that it has done its best.
With the current accuracy-related constants (NFast = 1000, NSlow = 10000), it will take
a few minutes to get there.

  Along the way, it will generate a dozen or so lines of output, showing that for each
of the 6 desired offsets, it is
      * first, trying to find two estimates, one too low and one too high, and
      * then, closing in until the bracket can't be made smaller.

  The line just above the "done" line will look something like
    [567,567] --> [-1,2]  [-2223,-2223] --> [0,1] [1131,1132] --> [16374,16404] [155,156] --> [-1,1]  [-25,-24] --> [0,3] [5,6] --> [0,4]
As will have been shown in interspersed header lines, the six groups making up this
line describe the optimum offsets for the X acceleration, Y acceleration, Z acceleration,
X gyro, Y gyro, and Z gyro, respectively.  In the sample shown just above, the trial showed
that +567 was the best offset for the X acceleration, -2223 was best for Y acceleration,
and so on.

  The need for the delay between readings (usDelay) was brought to my attention by Nikolaus Doppelhammer.
===============================================
*/

#include "I2Cdev.h"
#include "MPU6050.h"

#include "Wire.h"

#include "calibrate.h"

const char LBRACKET = '[';
const char RBRACKET = ']';
const char COMMA = ',';
const char BLANK = ' ';
const char PERIOD = '.';

const int iAx = 0;
const int iAy = 1;
const int iAz = 2;
const int iGx = 3;
const int iGy = 4;
const int iGz = 5;

const int usDelay = 3150; // empirical, to hold sampling to 200 Hz
const int NFast = 1000;   // the bigger, the better (but slower)
const int NSlow = 10000;  // ..

Calibration::Calibration(MPU6050 *mpu):accelgyro(mpu) {
}

void Calibration::GetSmoothed()
{
    int16_t RawValue[6];
    int i;
    long Sums[6];
    for (i = iAx; i <= iGz; i++)
    {
        Sums[i] = 0;
    }
    //    unsigned long Start = micros();

    for (i = 1; i <= N; i++)
    { // get sums
        accelgyro->getMotion6(&RawValue[iAx], &RawValue[iAy], &RawValue[iAz],
                             &RawValue[iGx], &RawValue[iGy], &RawValue[iGz]);
        if ((i % 500) == 0)
            Serial.print(PERIOD);
        delayMicroseconds(usDelay);
        for (int j = iAx; j <= iGz; j++)
            Sums[j] = Sums[j] + RawValue[j];
    } // get sums
      //    unsigned long usForN = micros() - Start;
      //    Serial.print(" reading at ");
      //    Serial.print(1000000/((usForN+N/2)/N));
      //    Serial.println(" Hz");
    for (i = iAx; i <= iGz; i++)
    {
        Smoothed[i] = (Sums[i] + N / 2) / N;
    }
} // GetSmoothed

void Calibration::SetOffsets(int TheOffsets[6])
{
    accelgyro->setXAccelOffset(TheOffsets[iAx]);
    accelgyro->setYAccelOffset(TheOffsets[iAy]);
    accelgyro->setZAccelOffset(TheOffsets[iAz]);
    accelgyro->setXGyroOffset(TheOffsets[iGx]);
    accelgyro->setYGyroOffset(TheOffsets[iGy]);
    accelgyro->setZGyroOffset(TheOffsets[iGz]);
} // SetOffsets

void Calibration::ShowProgress()
{
    Serial.print(BLANK);
    for (int i = iAx; i <= iGz; i++)
    {
        Serial.print(LBRACKET);
        Serial.print(LowOffset[i]),
            Serial.print(COMMA);
        Serial.print(HighOffset[i]);
        Serial.print("] --> [");
        Serial.print(LowValue[i]);
        Serial.print(COMMA);
        Serial.print(HighValue[i]);
        if (i == iGz)
        {
            Serial.println(RBRACKET);
        }
        else
        {
            Serial.print("],\n");
        }
    }
} // ShowProgress

void Calibration::PullBracketsIn()
{
    boolean AllBracketsNarrow;
    boolean StillWorking;
    int NewOffset[6];

    Serial.println("\nClosing in:");
    AllBracketsNarrow = false;
    StillWorking = true;
    while (StillWorking)
    {
        StillWorking = false;
        if (AllBracketsNarrow && (N == NFast))
        {
            SetAveraging(NSlow);
        }
        else
        {
            AllBracketsNarrow = true;
        } // tentative
        for (int i = iAx; i <= iGz; i++)
        {
            if (HighOffset[i] <= (LowOffset[i] + 1))
            {
                NewOffset[i] = LowOffset[i];
            }
            else
            { // binary search
                StillWorking = true;
                NewOffset[i] = (LowOffset[i] + HighOffset[i]) / 2;
                if (HighOffset[i] > (LowOffset[i] + 10))
                {
                    AllBracketsNarrow = false;
                }
            } // binary search
        }
        SetOffsets(NewOffset);
        GetSmoothed();
        for (int i = iAx; i <= iGz; i++)
        { // closing in
            if (Smoothed[i] > Target[i])
            { // use lower half
                HighOffset[i] = NewOffset[i];
                HighValue[i] = Smoothed[i];
            } // use lower half
            else
            { // use upper half
                LowOffset[i] = NewOffset[i];
                LowValue[i] = Smoothed[i];
            } // use upper half
        }     // closing in
        ShowProgress();
    } // still working

} // PullBracketsIn

void Calibration::PullBracketsOut()
{
    boolean Done = false;
    int NextLowOffset[6];
    int NextHighOffset[6];

    Serial.println("Expanding:");

    while (!Done)
    {
        Done = true;
        SetOffsets(LowOffset);
        GetSmoothed();
        for (int i = iAx; i <= iGz; i++)
        { // got low values
            LowValue[i] = Smoothed[i];
            if (LowValue[i] >= Target[i])
            {
                Done = false;
                NextLowOffset[i] = LowOffset[i] - 1000;
            }
            else
            {
                NextLowOffset[i] = LowOffset[i];
            }
        } // got low values

        SetOffsets(HighOffset);
        GetSmoothed();
        for (int i = iAx; i <= iGz; i++)
        { // got high values
            HighValue[i] = Smoothed[i];
            if (HighValue[i] <= Target[i])
            {
                Done = false;
                NextHighOffset[i] = HighOffset[i] + 1000;
            }
            else
            {
                NextHighOffset[i] = HighOffset[i];
            }
        } // got high values
        ShowProgress();
        for (int i = iAx; i <= iGz; i++)
        {
            LowOffset[i] = NextLowOffset[i];   // had to wait until ShowProgress done
            HighOffset[i] = NextHighOffset[i]; // ..
        }
    } // keep going
} // PullBracketsOut

void Calibration::SetAveraging(int NewN)
{
    N = NewN;
    Serial.print("Averaging ");
    Serial.print(N);
    Serial.println(" readings each time");
} // SetAveraging

int* Calibration::Calibrate()
{
    for (int i = iAx; i <= iGz; i++)
    {                  // set targets and initial guesses
        Target[i] = 0; // must fix for ZAccel
        HighOffset[i] = 0;
        LowOffset[i] = 0;
    } // set targets and initial guesses
    Target[iAz] = 16384;
    SetAveraging(NFast);

    PullBracketsOut();
    PullBracketsIn();

    Serial.println("-------------- Done --------------");

    return LowOffset;
} // setup
#endif