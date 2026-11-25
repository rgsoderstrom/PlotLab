
% integrate.m

% Simpson's 1/3 rule, from Google's AI Summary
% 	web article uses 0-based indexing

% Simpson's requires an even number of intervals 
%	i.e. length (samples) is odd

% for an odd number of intervals the last one
% is excluded from Simpson's and evaluated by
% a rectangular approximation

% returns:
%	results - intergration estimate
%	status  - 0 no error or warning
%			  1 low accuracy due to odd number of intervals
%			  2 error due to not enough input samples

function [results, status] = integrate (samples, dx)

status = 0; % 0 => good
results = 0;

N = length (samples);

if N < 5,
	status = 2; % error
else

	lengthIsOdd = (floor (N / 2) * 2 ~= N);

	first = 1; % always start with first samples

	if lengthIsOdd,
		last = N;
	else
		last = N - 1;
	end

	weightByOne  = [first, last];
	weightByFour = (first + 1) : 2 : (last - 1);
	weightByTwo  = (first + 2) : 2 : (last - 2);

	% disp (sprintf ('weight 1: %d', weightByOne));
	% disp (sprintf ('weight 4: %d', weightByFour));
	% disp (sprintf ('weight 2: %d', weightByTwo));
	% disp (sprintf ('Is odd:   %d', lengthIsOdd));

	summation = 0;
	summation = summation + sum (samples (weightByOne));
	summation = summation + sum (samples (weightByTwo))  * 2;
	summation = summation + sum (samples (weightByFour)) * 4;

	results = summation * dx / 3;

	if lengthIsOdd == false,
		results = results + mean (samples (N-1:N)) * dx;
		status = 1; % warning
	end
end





