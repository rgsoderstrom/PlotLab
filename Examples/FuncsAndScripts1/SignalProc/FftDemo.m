
% FftDemo.m

sr = 1 / 1000;
t = sr * (0 : 1023);

sig = exp (i * 2 * pi * 32 * t) + 2 * exp (i * 2 * pi * 200 * t);
%sig = sin (2 * pi * 32 * t) + 10 * sin (2 * pi * 128 * t);

spect = fft (sig, 1000);

clf ; hold on
plot (spect (1, :), 10 * log10 (spect (2, :)));
%plot (spect (1, :), spect (3, :) * 180 / pi);
