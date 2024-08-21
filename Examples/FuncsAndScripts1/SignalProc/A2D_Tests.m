
addpath C:\Users\rgsod\Documents\Visual Studio 2022\Projects\ArduinoSupport\SONAR\A2D_Tests\bin\Debug
        
clear
		
samples2 ; figure (1) ; clf ; title (''); hold off

samples = z';
fftResults = fft (samples, Fs);

freqScale  = fftResults (1, :);
magSquared = fftResults (2, :);

peakMag2 = 0;

for i=513:1023,
	if (peakMag2 < magSquared (i))
		peakMag2 = magSquared (i);
	end
end

magSquared = magSquared / peakMag2;
dB = 10 * log10 (magSquared);

plot (freqScale, dB);

for i=513:1023,
	A = (dB (i) > dB (i-1));
	B = (dB (i) > dB (i+1));
	C = (dB (i) > -55);
	
	if A && B && C
		disp (freqScale (i));
	end
end
