
% H&K Drill problems chap. 7

R = 1000;     % Ohms
L = 2; % Henrys
C = L / (4 * R * R); % Farads



a = 1 / (2 * R * C);
w0 = 1 / sqrt (L * C);

s1 = -a + sqrt (a ^ 2 - w0 ^ 2);
s2 = -a - sqrt (a ^ 2 - w0 ^ 2);

disp (sprintf ('alpha = %3.3f', a));
disp (sprintf ('w0 = %3.3f', w0));
disp (sprintf ('s1 = %3.3e', s1));
disp (sprintf ('s2 = %3.3e', s2));

A1 = 6.67;
A2 = 13.3;

t = (0 : 10 : 5000) * 1e-6;
v = A1 * exp (s1 * t) + A2 * exp (s2 * t);
plot (t * 1e3, v)
xlabel ('milliseconds');
