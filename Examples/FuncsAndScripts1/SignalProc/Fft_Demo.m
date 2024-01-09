
% Fft_Demo.m

sr = 1024; % samples per second
t = (0 : 1023) / sr;

%sig = exp (i * 2 * pi * 132 * t) + 1.414 * exp (i * 2 * pi * 200 * t);
sig = sin (2 * pi * 132 * t) + 1.414 * sin (2 * pi * 200 * t);

spect = fft (sig, sr);

clf ; hold on

plot (spect (1, :), 10 * log10 (spect (2, :)));
