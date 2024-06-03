```
using Unity.Barracuda;

public NNModel model1;
public NNModel model2;

void Update()
{
	if (Input.GetKeyDown(KeyCode.Alpha1))
	{
		Debug.Log("switched to model 1");
		SetModel("Walker", model1);
	}
	else if (Input.GetKeyDown(KeyCode.Alpha2))
	{
		Debug.Log("switched to model 2");
		SetModel("Walker", model2);
	}
}
```