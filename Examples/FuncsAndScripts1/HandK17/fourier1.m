
% fourier1 - section 17.2 equation 1

T = 0.4; % time for 1 period
f0 = 1 / T;

N = 65;
t = linspace (0, T, N);
dt = t (2) - t (1);

%%%%%%%%%%%%%%%%%%%%%%%%%%%

% % ramp
% rampLength = length (t) / 2;
% signal = linspace (0, 1, rampLength);

% padLength = length (t) - length (signal);
% signal    = [signal (zeros (1, padLength))];

%%%%%%%%%%%%%%%%%%%%%%%%%%%

% rectified sinusoid

signal = cos (2 * pi * f0 * t);

for n = 1:length (signal)
	if signal (n) < 0,
		signal (n) = 0;
	end
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%

clf ; plot (t, signal, 'r') ; hold on


% DC component
a0 = (1/T) * integrate (signal, dt);

% Harmonics
K = 1:3;

a = zeros (1, length (K)); % cos coefficients
b = zeros (1, length (K)); % sin    "

for k = K,
	w = cos (k * 2 * pi * f0 * t);
	v = signal .* w;
	a (k) = (2 / T) * integrate (v, dt);
end

for k = K,
	w = sin (k * 2 * pi * f0 * t);
	v = signal .* w;
	b (k) = (2 / T) * integrate (v, dt);
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Approximate input by adding harmonics

reconstructed = a0 * ones (1, length (t));

for k = K,
	v = a (k) * cos (k * 2 * pi * f0 * t);
	w = b (k) * sin (k * 2 * pi * f0 * t);
	reconstructed = reconstructed + v + w;
end

plot (t, reconstructed, 'g');







