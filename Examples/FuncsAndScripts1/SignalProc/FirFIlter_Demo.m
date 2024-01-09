
% FirFilterDemo.m

sampleRate = 2000; % samples / sec
cutoff = 250; % Hz
dec = 2; % decimation

[h, coefs] = CreateLPF (sampleRate, cutoff);
disp (sprintf ('%d coefficients', length (coefs)));

t = 0 : (1/sampleRate) : 0.5;

sig = sin (2 * pi * 100 * t) + sin (2 * pi * 400 * t);
sig2 = RunFilter (h, sig, dec);

figure ; clf ; hold on ; title ('Red: input, green: filtered output');

spect = fft (sig (1:512), sampleRate);
plot (spect (1, :), 10 * log10 (spect (2,:)), 'r')

start = length (coefs);

spect = fft (sig2 (start:start+255), sampleRate / dec);
plot (spect (1, :), 10 * log10 (spect (2,:)), 'g')