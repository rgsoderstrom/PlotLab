
% UltraSonic_Bandpass.m 
%	- model of ultrasonic sonar with bandpass sampling

Fc = 40810; % center freq of signal
B = 5000;

Fs = 19150;

t1 = 0 : (1 / Fs) : 0.25;

Signal = sin (2 * pi * Fc * t1);

h1 = CreateLPF (Fs, B);
Baseband = RunFilter (h1, Signal, 2);
tb = t1 (1:2:end); % decimate time

clf ; hold on
%plot (t1, Signal);
%plot (tb, Baseband, 'r');

LoI = sin (2 * pi * 2500 * tb);
LoQ = cos (2 * pi * 2500 * tb);

h2 = CreateLPF (Fs / 2, 2000);
h3 = CreateLPF (Fs / 2, 2000);

s3 = LoI .* Baseband;
s4 = LoQ .* Baseband;

s5 = RunFilter (h2, s3, 4);
s6 = RunFilter (h3, s4, 4);

plot (s5, 'r');
plot (s6);


