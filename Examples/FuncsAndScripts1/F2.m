
% i am a comment
% me too

% function [p, q] = f2 (aa, bb)

function [sum, prod] = f2 (aa, ...  % some of the line
                           bb)      % some more

A = 10;

sum  = aa + bb + A;
prod = aa * bb * A;

if sum > 20,
	disp ('in f2, if stmt over writing sum');
	sum = 123;
	return
end

disp ('in f2, fall through over writing sum');
sum = 456;

% disp (sprintf ('print inside F2, A = %d', A));


