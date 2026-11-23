
% integrate trig functions

x1 = 0;
x2 = 4;
N  = 13; % number of points, number
        % of intervals is one less

figure (1) ; clf ;

x = linspace (x1, x2, N);
dx = (x2 - x1) / (N - 1); % interval width

y = x .* x;

plot (x, y) ; hold on ; plot (x, y, '*', 'r', 'Size', dx / 4);
[res, stat] = integrate (y, dx);
disp (sprintf ('Results = %f', res));
disp (sprintf ('Status  = %f', stat));




% T = 2*pi; % time for 1 period
% dt = 2 * pi / 31;
% t = 0 : dt : T/2;

% W0 = 2 * pi / T;
% n = 1;

% sine   = sin (n * W0 * t); 
% cosine = 1;%cos (n * W0 * t);

% signal = sine .* cosine;

% clf ; plot (t, signal);
% a = integrate (signal, dt)
