
% fourier1 - section 17.2 equation 1

T = 3; % time for 1 period
f0 = 1 / T;
dt = 0.05;
t = 0 : dt : T;

% ramp
rampLength = length (t) / 2;
signal = linspace (0, 1, rampLength);

padLength = length (t) - length (signal);
signal    = [signal (zeros (1, padLength))];
plot (t, signal)

% DC component
intg = integrate (signal, dt);
a0 = (1/T) * intg;

% Harmonics
K = 1:6;

a = zeros (1, length (K)); % cos coefficients
b = zeros (1, length (K)); % sin    "

for k = K,
	w = cos (k * 2 * pi * f0 * t);
	v = signal .* w;
	[coef, quality] = (2 / T) * integrate (v, dt);
	a (k) = coef;
end

for k = K,
	w = sin (k * 2 * pi * f0 * t);
	v = signal .* w;
	[coef, quality] = (2 / T) * integrate (v, dt);
	b (k) = coef;
end







