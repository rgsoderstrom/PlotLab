
clear

t = linspace (0, 8 * pi, 200);

% x = cos (t);
% y = sin (t);
% z = cos (2 * t);

% pts = [x ; y ; z];

% plot (pts);


x = t + sqrt (3) * sin (t);
y = t * sqrt (3) - sin (t);
z = 2 * cos (t);

pts = [x ; y ; z];

plot (pts);
