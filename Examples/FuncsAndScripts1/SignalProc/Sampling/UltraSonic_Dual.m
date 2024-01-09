
% UltraSonic_Dual.m 
%	- model analog and FPGA processing
%		- analog conversion to real signal, external to FPGA
%		- digital conversion to baseband inside of FPGA
%	

Fs1 = 120000; % high sample rate to model analog processing
N = 3000;
t1 = [0 : N-1] / Fs1;

fftSize = 1024;
res = Fs1 / fftSize; % fft resolution

%RxFreq      = round (40800 / res) * res; % round to nearest bin-centered fre
%FirstLoFreq = round (30000 / res) * res;

RxFreq      = 40800 + 40;
FirstLoFreq = 30000;



disp (sprintf ('Received freq = %f', RxFreq));
disp (sprintf ('First local osc freq = %f', FirstLoFreq));
%disp (sprintf ('difference = %f', RxFreq - LoFreq));

w = 2 * pi * RxFreq;
RxSignal = sin (w * t1) + 0.01 * (rand (1, N) - 0.5);

w = 2 * pi * FirstLoFreq;
Lo1 = sin (w * t1);
%Lo = exp (j * w * t);

%
% display spectrum of received signal and first local osc before mixing
%
figure (1) ; clf ; hold on ; set (1, 'position', [800 30 800 500]) ; title ('Mixer inputs. red: received, green: analog LO');

spect = fft (RxSignal (1:fftSize), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)), 'r');

spect = fft (Lo1 (1:fftSize), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)), 'g');


%
% after multiply stage of analog mixer
%

figure (2); clf ; hold on ; set (2, 'position', [1000 80 800 500]) ; title ('After analog multiplication');

s1    = Lo1 .* RxSignal;
spect = fft (s1 (1:fftSize), Fs1);
plot (spect (1,:), 10 * log10 (spect (2,:)));

%
% after filter
%
[h1, coefs] = CreateLPF (Fs1, 15000);
size (coefs)

dec = 2;

s2 = RunFilter (h1, s1, dec);

figure (3); clf ; hold on ; set (3, 'position', [1200 130 800 500])
title ('After filter, at A/D input');

start = 60;

spect = fft (s2 (start:start+(fftSize/dec)-1), Fs1 / dec);
plot (spect (1,:), 10 * log10 (spect (2,:)));

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Processing inside of FPGA

Lo2Freq = 10800;
w = 2 * pi * Lo2Freq;
Lo2 = exp (j * w * t);

s3 = conj (Lo2) * s2;















