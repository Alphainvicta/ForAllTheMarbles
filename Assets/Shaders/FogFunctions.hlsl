#ifndef FOG_FUNCTIONS_INCLUDED
#define FOG_FUNCTIONS_INCLUDED


float CalculateFog_float(float depth, float fogStart, float fogEnd, float fogDensity, out float fogFactor)
{
    
    fogFactor = saturate((depth - fogStart) / (fogEnd - fogStart));
    
    
    return fogFactor * fogDensity;
}

#endif