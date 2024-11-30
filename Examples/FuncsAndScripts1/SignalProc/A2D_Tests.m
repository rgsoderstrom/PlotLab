
addpath C:\Users\rgsod\Documents\Visual Studio 2022\Projects\ArduinoSupport\SONAR\A2D_Tests\bin\Debug
        
clear
		
samples3 ; figure ; clf ; title (''); hold off

samples = z';

% window
L = length (samples);
phase = linspace (0, 2 * pi, L);
win = 0.5 * (1 - cos (phase));

dc = mean (samples);
samples = samples - dc;


fftResults = fft (samples, Fs);
%fftResults = fft (win .* samples, Fs);

freqScale  = fftResults (1, :);
magSquared = fftResults (2, :);
phase      = fftResults (3, :);

peakMag2 = 0;

for i=513:1023,
	if (peakMag2 < magSquared (i))
		peakMag2 = magSquared (i);
	end
end

magSquared = magSquared / peakMag2;
dB = 10 * log10 (magSquared);

plot (freqScale, dB);

%return

for i=513:1023,
	A = (dB (i) > dB (i-1));
	B = (dB (i) > dB (i+1));
	C = (dB (i) > -55);
	
	if A && B && C
		%dd = [(freqScale (i)) (sqrt (magSquared (i) * peakMag2)) (phase (i))];
		%disp (dd);
	    disp (freqScale (i));
	end
end
