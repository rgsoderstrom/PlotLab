
% FftDemo.m

sr = 1024; % samples per second
t = (0 : 511) / sr;

%sig = exp (i * 2 * pi * 32 * t) + 2 * exp (i * 2 * pi * 200 * t);
sig = sin (2 * pi * 32 * t) + 1.414 * sin (2 * pi * 128 * t);

spect = fft (sig, sr);

clf ; hold on

dB = 10 * log10 (spect (2, :));
m = max (dB);

plot (spect (1, :), dB - m);
axis ([-200 200 -8 1]);

%plot (spect (1, :), spect (3, :) * 180 / pi);
