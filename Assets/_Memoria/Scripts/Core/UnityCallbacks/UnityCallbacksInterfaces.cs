using UnityEngine;

namespace UnityCallbacks
{
	public interface IOnValidate
	{
		void OnValidate();
	}

	public interface IOnEnable
	{
		void OnEnable();
	}

	public interface IAwake
	{
		void Awake();
	}

	public interface IStart
	{
		void Start();
	}

	public interface IFixedUpdate
	{
		void FixedUpdate();
	}

	public interface IUpdate
	{
		void Update();
	}

	public interface IOnGui
	{
		void OnGui();
	}

	public interface IOnDrawGizmos
	{
		void OnDrawGizmos();
	}

	public interface IOnTriggerEnter
	{
		void OnTriggerEnter(Collider other);
	}

	public interface IOnTriggerStay
	{
		void OnTriggerStay(Collider other);
	}

	public interface IOnTriggerExit
	{
		void OnTriggerExit(Collider other);
	}

	public interface IOnMouseOver
	{
		void OnMouseOver();
	}
}
