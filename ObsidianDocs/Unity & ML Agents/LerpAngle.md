```
public static float LerpAngle(float a, float b, float t)

{
float num = Repeat(b - a, 360f);

if (num > 180f)
{
num -= 360f;
} 

return a + num * Clamp01(t);

}
```

First with the repeat function the repeated tourns get cut off so that the resulting value is in the range of 0 to 360

Then if the the value is larger than 180 the 360 is subtracted from the value

Then the value gets lerped like in the normal lerp function