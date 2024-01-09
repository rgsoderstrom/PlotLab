
%
% BandPassSampling.m
%

% center frequency and bandwidth of the signal being sampled
Fc = 40800;
B  = 5000;

%
% calculate sampling frequency to move one image of original 
% to (or at least near) baseband
%
m = 4; % number of images produced between -Fc and Fc
Fs = (2 * Fc - B) / m; 

disp (sprintf ('Fs = %f;', Fs));

% plot the results
clf ; hold on

N = m + 1;

for i = (1 - N) : 1 : (N - 1),
	if i == 0,
		color = 'g';
	else
		color = 'g.';
	end
	
	RealSpectMarker (Fc, i * Fs, B, color);	
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% axis lines
xl = -1.2 * Fc;
xr = -1 * xl;

plot ([xl xr], [0 0]);
plot ([0 0], [0 3]);

% draw line at Nyquist freq, where image start to alias
Fn = 2 * Fs;
plot ([Fn Fn], [0 2], '.');
text (Fn, 2.2, 'Nyquist folding frequency');

% adjust plot size
a = zeros (1, 4);
a (1) = -1.75 * Fc;
a (2) =  1.75 * Fc;
a (3) = -0.1;
a (4) = 4;
axis (a);











