
% test driver for integrate function

t0 = 0;
T  = 2*pi;
N  = 65; % number of points, must be odd and 5 or more
	
figure (1) ; clf ;

x = linspace (t0, T, N);
dx = (T - t0) / (N - 1); % interval width

% y = x .* x;

n = 1;
k = 1;

y1 = sin (n * x);
y2 = sin (k * x);
y  = y1 .* y2;

clear res stat

clf 
plot (x, y) ; hold on ; plot (x, y, '*', 'r', 'Size', dx / 6);
res = integrate (y, dx);
disp (sprintf ('Results = %3.3f', res));
