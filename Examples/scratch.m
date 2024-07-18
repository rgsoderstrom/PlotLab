
%
% scratch.m
%

clear

ClockFrequency  = 50e6;
DelayTime       = 0.003;
BlankingVoltage = 0.025;
InitialVoltage  = 0.125;
FinalVoltage    = 1.325;
RiseTime        = 0.030;

CountsPerVolt    = 1023 / 2.048;
StartDelayClocks = DelayTime * ClockFrequency;
InitialCounts    =  InitialVoltage * CountsPerVolt;
BlankingCounts   = BlankingVoltage * CountsPerVolt;    
FinalCounts      =    FinalVoltage * CountsPerVolt;    
RampCounts       = FinalCounts - InitialCounts;
RampCountRate    = RampCounts / RiseTime;                          
ClockDivisor     = ClockFrequency / RampCountRate;

sprintf ('BlankingCounts %f', BlankingCounts)
sprintf ('InitialCounts  %f', InitialCounts)
sprintf ('FinalCounts    %f', FinalCounts)
sprintf ('RampCounts     %f', RampCounts)
sprintf ('RampCountRate  %f', RampCountRate)
sprintf ('StartDelayClocks %f', StartDelayClocks)
sprintf ('ClockDivisor     %f', ClockDivisor)


