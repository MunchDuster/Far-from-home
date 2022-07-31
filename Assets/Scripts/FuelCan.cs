using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;

public class FuelCan : Pickupable
{
    public Transform nozzle;
	public Transform handle;
	public Flow flow;
	public Slider fullnessSlider;
	public Image fullnessSliderFillImage;
}
