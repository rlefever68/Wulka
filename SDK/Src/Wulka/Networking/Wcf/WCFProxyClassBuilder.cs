using System;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Wulka.Core;

namespace Wulka.Networking.Wcf
{
	/// <summary>
	/// Use a bit of reflection and code emiting to emit a nice proxy class that inherits from ClientBase<TInterface>, TInterface.
	/// The emited class follows the recommended ClientBase pattern.
	/// </summary>
	/// <typeparam name="TInterface"></typeparam>
	internal class WCFProxyClassBuilder<TInterface>
		: AbstractClassBuilder<TInterface> where TInterface : class
	{
		public WCFProxyClassBuilder()
			: base(typeof (ClientBase<TInterface>))
		{
		}


        protected override void GenerateConstructor(TypeBuilder builder)
        {
            base.GenerateConstructor(builder);
            // Define the constructor
            Type[] constructorParameters = new[] { typeof(Binding), typeof(EndpointAddress) };
            ConstructorBuilder constructorBuilder = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName, CallingConventions.Standard, constructorParameters);
            ILGenerator iLGenerator = constructorBuilder.GetILGenerator();

            iLGenerator.Emit(OpCodes.Ldarg_0); // this
            iLGenerator.Emit(OpCodes.Ldarg_1); // load the param
            iLGenerator.Emit(OpCodes.Ldarg_2); // load the param2

            // Call the base constructor
            ConstructorInfo originalConstructor = baseClassType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, constructorParameters, null);
            iLGenerator.Emit(OpCodes.Call, originalConstructor);

            iLGenerator.Emit(OpCodes.Ret);
        }


        






		/// <summary>
		/// Generate the contents of the method. This will generate:
		/// 
		/// ....
		/// return Channel.MethodName(params);
		/// ...
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <param name="parameterTypes"></param>
		/// <param name="iLGenerator"></param>
		protected override void GenerateMethodImpl(MethodInfo method, Type[] parameterTypes, ILGenerator iLGenerator)
		{
			iLGenerator.Emit(OpCodes.Ldarg_0); // this

			// Get the details Property of the ClientBase
			MethodInfo channelProperty = GetMethodFromBaseClass("get_Channel");
			// Get the channel: "base.Channel<TInterface>."
			iLGenerator.EmitCall(OpCodes.Call, channelProperty, null);

			// Prepare the parameters for the call
			ParameterInfo[] parameters = method.GetParameters();
			for (int index = 0; index < parameterTypes.Length; index++)
			{
				iLGenerator.Emit(OpCodes.Ldarg, (((short) index) + 1));
			}

			// Call the Channel via the interface
			iLGenerator.Emit(OpCodes.Callvirt, method);

			// Thanks, all done
			iLGenerator.Emit(OpCodes.Ret);
		}
	}
}