#pragma strict

var MaxEffects : int = 9;

function GetRandomEffect() {	
	var baseEffect : AtomizerEffectGroup = Atomizer.CreateEffectGroup();
	var combine = (Random.value > 0.5f);
	var numberOfEffects : int = Random.Range(1, 4);
	
	for (var i = 0; i < numberOfEffects; ++i) {	
		var selection : int = Random.Range(0, MaxEffects);	
		if (selection == 0) {
			var disintegrate = new Disintegrate();
			disintegrate.Duration = Random.Range(1.0f, 5.0f);
			disintegrate.FadeColour = new Color(Random.value, Random.value, Random.value, Random.value);
			if (combine) {
				baseEffect = baseEffect.Combine(disintegrate);
			} else {
				baseEffect = baseEffect.Chain(disintegrate);
			}		
		} else if (selection == 1) { 
			var percolate = new Percolate();
			percolate.PercolationTime = Random.Range(1.0f, 4.0f);
			percolate.PercolationSpeed = Random.Range(0.5f, 4.0f);
			percolate.Duration = 2.0f * percolate.PercolationTime;
			if (combine) {
				baseEffect = baseEffect.Combine(percolate);
			} else {
				baseEffect = baseEffect.Chain(percolate);
			}			
		} else if (selection == 2) { 
			var breakAndReform = new BreakAndReform();
			breakAndReform.BreakApartSpeed = Random.Range(1.0f, 8.0f);
			breakAndReform.BreakPhaseDuration = Random.Range(1.0f, 3.0f);
			breakAndReform.ReformPhaseDuration = Random.Range(1.0f, 3.0f);
			if (combine) {
				baseEffect = baseEffect.Combine(breakAndReform);
			} else {
				baseEffect = baseEffect.Chain(breakAndReform);
			}	
		} else if (selection == 3) { 
			var swapColours = new SwapColours();
			swapColours.Duration = Random.Range(1.0f, 4.0f);
			swapColours.ColourSwapMode = ColourSwapMode.Inverted;
			if (combine) {
				baseEffect = baseEffect.Combine(swapColours);
			} else {
				baseEffect = baseEffect.Chain(swapColours);
			}	
		} else if (selection == 4) { 
			var pulse = new Pulse();
			pulse.PulseLength = Random.Range(0.3f, 1.0f);
			pulse.PulseStrength = Random.Range(0.2f, 0.8f);
			pulse.Duration = 4.0f * pulse.PulseLength;
			if (combine) {
				baseEffect = baseEffect.Combine(pulse);
			} else {
				baseEffect = baseEffect.Chain(pulse);
			}	
		} else if (selection == 5) { 
			var vacuum = new Vacuum();
			vacuum.VacuumPoint = new Vector3(
				transform.position.x + Random.Range(-1.0f, 1.0f),
				transform.position.y + Random.Range(-1.0f, 1.0f),
				transform.position.z + Random.Range(-1.0f, 1.0f));
			vacuum.MoveSpeed = Random.Range(0.1f, 3.0f);
			vacuum.Duration = 5.0f;
			if (combine) {
				baseEffect = baseEffect.Combine(vacuum);
			} else {
				baseEffect = baseEffect.Chain(vacuum);
			}	
		} else if (selection == 6) { 
			var warp = new Warp();
			warp.WarpTime = Random.Range(1.0f, 2.0f);
			warp.TargetPoint = new Vector3(
				transform.position.x + Random.Range(-1.0f, 1.0f),
				transform.position.y + Random.Range(-1.0f, 1.0f),
				transform.position.z + Random.Range(-1.0f, 1.0f));
			warp.StartScale = Random.Range(0.5f, 3.0f);
			if (combine) {
				baseEffect = baseEffect.Combine(warp);
			} else {
				baseEffect = baseEffect.Chain(warp);
			}			
		} else if (selection == 7) { 
			var smolder = new Smolder();
			smolder.SmolderColour = new Color(Random.value, Random.value, Random.value, Random.value);
			smolder.BurntColour = new Color(Random.value, Random.value, Random.value, Random.value);
			smolder.EndColour = Color.clear;
			smolder.SmolderDuration = Random.Range(0.5f, 2.5f);
			smolder.BurntDuration = Random.Range(0.5f, 2.5f);
			smolder.Smoothness = Random.Range(0.1f, 0.9f);
			smolder.DispersionForce = Random.Range(0.0f, 4.0f);
			smolder.SmolderSpeed = Random.Range(0.1f, 2.0f);
			smolder.Source = Input.mousePosition;	
			if (combine) {
				baseEffect = baseEffect.Combine(smolder);
			} else {
				baseEffect = baseEffect.Chain(smolder);
			}		
		} else if (selection == 8) { 
			var jiggle = new Jiggle();
			jiggle.Duration = Random.Range(1.0f, 4.0f);
			jiggle.Noise = Random.value;
			if (combine) {
				baseEffect = baseEffect.Combine(jiggle);
			} else {
				baseEffect = baseEffect.Chain(jiggle);
			}	
		} 
	}
	return baseEffect;
}

function Play() {
	var index : int = UnityEngine.Random.Range(0, MaxEffects);	
			
	var effect : AtomizerEffectGroup = GetRandomEffect();
	Atomizer.Atomize(gameObject, effect, OnFinish);
	gameObject.GetAllComponents.<Renderer>().ForEach(function(item) { item.enabled = false; });
}

function OnMouseUpAsButton() {
	Play();
}
	
function OnFinish() {
	gameObject.GetAllComponents.<Renderer>().ForEach(function(item) { item.enabled = true; });
}