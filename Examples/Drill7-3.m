
% H&K Drill problem 7-3

R = 400;     % Ohms
C = 0.01e-6; % Farads
L = 10e-3; % Henrys

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

t = (0 : 0.01 : 2) * 1e-5;
v = A1 * exp (s1 * t) + A2 * exp (s2 * t);
plot (t * 1e5, v)
xlabel ('x 10 uSec');
