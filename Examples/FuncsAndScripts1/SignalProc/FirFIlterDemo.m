
% FirFilterDemo.m

h = CreateLPF (2000, 100);
t = 0 : (1/2000) : 0.3;
sig = sin (2 * pi * 30 * t) + sin (2 * pi * 130 * t);
sig2 = RunFilter (1, sig);

clf ; hold on
plot (sig)
plot (sig2, 'r')
