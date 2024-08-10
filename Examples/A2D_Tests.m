
addpath C:\Users\rgsod\Documents\Visual Studio 2022\Projects\ArduinoSupport\SONAR\A2D_Tests\bin\Debug
samples1

Fs = 19150;
dt = 1 / Fs;
t = (0 : 1023) * dt;

plot (z (:, 1), z (:,2))
plot (z (:, 1), z (:,2), '*')

% f = 2 * Fs + 18;
% s = 500 + 400 * sin (2 * pi * 19168 * t);
% plot (s, '*r');

% zz = z (:, 2);
% zz = zz / 4;
% zz = round (zz);

% plot (zz);
% plot (zz, '*');


