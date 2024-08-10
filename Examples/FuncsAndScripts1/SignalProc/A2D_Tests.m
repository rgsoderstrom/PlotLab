
addpath C:\Users\rgsod\Documents\Visual Studio 2022\Projects\ArduinoSupport\SONAR\A2D_Tests\bin\Debug
        
clear
		
samples3

Fs = 19150;
dt = 1 / Fs;
t = (0 : 1023) * dt;

p = 100:300;
plot (z (p, 1), z (p,2))
plot (z (p, 1), z (p,2), 'r*', 'Size', 0.4);

f = 2 * Fs + 18;
s = 500 + 400 * sin (2 * pi * 19168 * t);
%plot (s, '*r');

% zz = z (:, 2);
% zz = zz / 4;
% zz = round (zz);

% plot (zz);
% plot (zz, '*');


