
% i am a comment
% me too

% function [p, q] = f2 (aa, bb, cc)

function [p, q] = f2 (aa, ...  % some of the line
                      bb, ...  % some more
					  cc)

A = -20;

p = aa + bb + cc + A;
q = aa * bb * cc;

disp (sprintf ('print inside F2, A = %d', A));


