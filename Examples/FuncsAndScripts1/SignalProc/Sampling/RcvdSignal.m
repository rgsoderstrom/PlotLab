
% RcvdSignal.m

function A = RcvdSignal (t)

freqs = [407 409];

A = sin (2 * pi * freqs (1) * t) + sin (2 * pi * freqs (2) * t);

