
% integrate trig functions

x1 = 0;
x2 = pi;
N  = 31; % number of points, number
         % of intervals is one less

figure (1) ; clf ;

x = linspace (x1, x2, N);
dx = (x2 - x1) / (N - 1); % interval width

% y = x .* x;
y = sin (x);

clear res stat

plot (x, y) ; hold on ; plot (x, y, '*', 'r', 'Size', dx / 4);
[res, stat] = integrate (y, dx);
disp (sprintf ('Results = %f', res));
disp (sprintf ('Status  = %f', stat));
