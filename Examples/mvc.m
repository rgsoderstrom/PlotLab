
% mvc.m - scratch file for MVC problems

figure (1) ; clf ; hold on

t1 = linspace (1, 2, 20);
t2 = linspace (0, pi/2, 20);

plot (t1, t1 .* t1, 'r+', 'Size', 0.1);

x = 1 + sin (t2);
y = 1 + 2 * sin (t2) + (sin (t2)) .^ 2;
plot (x, y, 'x', 'Size', 0.1);

axis equal
