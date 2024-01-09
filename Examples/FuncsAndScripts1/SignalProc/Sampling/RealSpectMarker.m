
% RealSpectMarker.m - draw a shape at (sampling frequency) +- (center frequency)

function [] = RealSpectMarker (Fc, Fs, BW, color)

h = 1; % height

x1 = Fs + Fc - BW / 2;
x2 = Fs + Fc + BW / 2;

y1 = 0.8 * h;
y2 = h;

plot ([x1 x1], [0  y1], color);
plot ([x1 x2], [y1 y2], color);
plot ([x2 x2], [y2  0], color);

x1 = Fs - Fc - BW / 2;
x2 = Fs - Fc + BW / 2;


plot ([x1 x1], [0  y2], color);
plot ([x1 x2], [y2 y1], color);
plot ([x2 x2], [y1  0], color);

