
% BPS_Demo.m - Bandpass sampling, using ultrasonic sonar numbers

% frequencies scaled by 1/100

Fc = 408;
Fs = 1024; % 191.5;

t = 0 : (1/Fs) : 1;

signal = RcvdSignal (t);
plot (t, signal);
