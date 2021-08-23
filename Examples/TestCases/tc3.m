

% tc3.m

contour (ContourZ1, [0.1 : 0.4 : 5], -2.5, 2.5, -2, 2)

figure
x = [1, 2, 3, 1, 2, 3, 1, 2, 3];
y = [3, 3, 3, 2, 2, 2, 1, 1, 1];
pts = [x ; y];
vec = pts / 4;
whos
quiver (pts, vec);



figure

x = [-5:0.1:5];  % comment,
y = x .^ 3;

plot (x, y, '*');
plot (x, y, 'r.');

hold off
yy = x .^ 2;
figure
plot (x, yy)


