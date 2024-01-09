
% UltraSonic_Bandpass.m 
%	- model of ultrasonic sonar with bandpass sampling

Fc = 40832; % freq of received signal
Fs = 19150; % see BandPassSampling.m for this number

t = 0 : (1 / Fs) : 0.25;

Samples = sin (2 * pi * Fc * t); % note is sampled below Nyquist rate

spect = fft (Samples (1:1024), Fs);
plot (spect (1, :), 10 * log10 (spect (2, :)));


[h1, coefs1] = CreateLPF (Fs, 5000);

dec1 = 2;
s1 = RunFilter (h1, Samples, dec1);
t1 = t (1:dec1:end); % also decimate time scale

clf ; hold on
%plot (t, Samples);
%plot (t1, s1, 'r');
%return


LoI = sin (2 * pi * 2500 * t1);
LoQ = cos (2 * pi * 2500 * t1);

[h2, coefs2] = CreateLPF (Fs / dec1, 2000);
h3 = CreateLPF (Fs / dec1, 2000);

s3 = LoI .* s1;
s4 = LoQ .* s1;

dec2 = 4;
s5 = RunFilter (h2, s3, dec2);
s6 = RunFilter (h3, s4, dec2);

%return

s56 = s5 (79 : 590) + sqrt (-1) * s6 (79 : 590);
spect = fft (s56, Fs / (dec1 * dec2));

disp (sprintf ('FFT resolution = %f Hz', Fs / (dec1 * dec2) / 512));
%s56 = s5 + sqrt (-1) * s6;
%spect = fft (s56 (1 : 512));




plot (spect (1, :), 10 * log10 (spect (2, :)));
%plot (s5, 'r');
%plot (s6);


