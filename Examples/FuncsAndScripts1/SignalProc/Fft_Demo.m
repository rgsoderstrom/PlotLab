
% Fft_Demo.m

fftSize = 8192;

sr = 1024; % samples per second
t = (0 : (fftSize - 1)) / sr;

%sig = exp (i * 2 * pi * 132 * t) + 1.414 * exp (i * 2 * pi * 200 * t);
%sig = sin (2 * pi * 132 * t) + 1.414 * sin (2 * pi * 200 * t) + rand (1, length (t)) - 0.5;
sig = sin (2 * pi * 132 * t); % + 1.414 * sin (2 * pi * 200 * t) + rand (1, 1024) - 0.5;

spect = fft (sig, sr);

clf ; hold on

%plot (spect (1, :), spect (2, :));
plot (spect (1, :), 10 * log10 (spect (2, :)));

2 * max (spect (2,:))
