
% UltraSonic_Conventional.m 
%
%   - real signals
%	- MixAndLPF_Demo with numbers changed

Fs1 = 120000; % sample rate for received signal
t1 = [0 : 3000] / Fs1;

fftSize = 1024;
res = Fs1 / fftSize; % fft resolution

RxFreq = round (40200 / res) * res; % round to nearest bin-centered fre
LoFreq = round (40000 / res) * res;



RxSignal = sin (2 * pi * RxFreq * t1);
Lo       = sin (2 * pi * LoFreq * t1);

%
% display spectrum of received signal and local osc before mixing
%
figure (1) ; clf ; hold on ; set (1, 'position', [800 30 800 500]) ; title ('Mixer inputs: red - received, green - LO');

spect = fft (RxSignal (500:500+fftSize-1), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)), 'r');

spect = fft (Lo (500:500+fftSize-1), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)), 'g');

%
% after multiply stage of mixer
%
s1 = Lo .* RxSignal;
figure (2); clf ; hold on ; set (2, 'position', [1000 80 800 500]) ; title ('After multiplication');

spect = fft (s1 (500:500+fftSize-1), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)));

%
% after filter
%
[h, coefs] = CreateLPF (Fs1, 3000);

dec = 4;
s2 = RunFilter (h, s1, dec);

figure (3); clf ; hold on ; set (3, 'position', [1200 130 800 500]) ; title ('After filter');

spect = fft (s2 (100:100+(fftSize/dec)-1), Fs1 / dec);
plot (spect (1,:), 10 * log10 (spect (2,:)));




