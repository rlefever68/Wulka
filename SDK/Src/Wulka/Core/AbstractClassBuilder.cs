using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Wulka.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    /// <remarks></remarks>
	public abstract class AbstractClassBuilder<TInterface> : ITypeBuilder
		where TInterface : class
	{
        /// <summary>
        /// 
        /// </summary>
		private const string ASSEMBLY_NAME = "Wulka.";
        /// <summary>
        /// 
        /// </summary>
		private const MethodAttributes defaultMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot;

        /// <summary>
        /// 
        /// </summary>
		private static bool saveGeneratedAssembly = false; // set to true to save the generated assembly to the disk

        /// <summary>
        /// 
        /// </summary>
		private AssemblyBuilder assemblyBuilder;
        /// <summary>
        /// 
        /// </summary>
		private AssemblyName assemblyName;
        /// <summary>
        /// 
        /// </summary>
		private ModuleBuilder moduleBuilder;

        /// <summary>
        /// 
        /// </summary>
		protected readonly Type baseClassType;

        /// <summary>
        /// 
        /// </summary>
		private string generatedAssemblyName;
        /// <summary>
        /// 
        /// </summary>
		private string generatedClassName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClassBuilder&lt;TInterface&gt;"/> class.
        /// </summary>
        /// <param name="baseClassType">Type of the base class.</param>
        /// <remarks></remarks>
		protected AbstractClassBuilder(Type baseClassType)
		{
			this.baseClassType = baseClassType;
		}

		#region Properties

        /// <summary>
        /// Gets the name of the generated class.
        /// </summary>
        /// <remarks></remarks>
		public string GeneratedClassName
		{
			get { return generatedClassName; }
		}

		#endregion

        /// <summary>
        /// Generates the assembly.
        /// </summary>
        /// <remarks></remarks>
		private void GenerateAssembly()
		{
			try
			{
				if (assemblyBuilder == null)
				{
					assemblyName = new AssemblyName();
					assemblyName.Name = generatedAssemblyName;
					assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, saveGeneratedAssembly ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
					moduleBuilder = saveGeneratedAssembly ? assemblyBuilder.DefineDynamicModule(generatedAssemblyName, generatedAssemblyName + ".dll") : assemblyBuilder.DefineDynamicModule(generatedAssemblyName);
				}

				if (moduleBuilder == null)
				{
					throw new ApplicationException("Could not generate module builder");
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not generate assembly " + generatedAssemblyName, ex);
			}
		}


		/// <summary>
		/// Generate a type declare like
		/// public class MyType : ClientBase < ITheInterface >, ITheInterface 
		/// that implements the interface for us
		/// </summary>
		/// <returns></returns>
		private Type GenerateTypeImplementation()
		{
			TypeBuilder builder;
			try
			{
				builder = moduleBuilder.DefineType(generatedAssemblyName + "." + generatedClassName, TypeAttributes.Public, baseClassType);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not DefineType " + generatedClassName + " : " + baseClassType, ex);
			}
			if (builder == null)
			{
				throw new ApplicationException("Could not DefineType " + generatedClassName + " : " + baseClassType + " interface " + typeof (TInterface).FullName);
			}
			try
			{
				builder.AddInterfaceImplementation(typeof (TInterface)); // implement the interface
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not DefineType " + generatedClassName + " : " + baseClassType + " interface " + typeof (TInterface).FullName, ex);
			}

			GenerateConstructor(builder);

			GenerateMethodImpl(builder);

			try
			{
				Type generatedType = builder.CreateType();
				return generatedType;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not CreateType " + generatedClassName + " : " + baseClassType + " interface " + typeof (TInterface).FullName, ex);
			}
		}

		public void SaveGeneratedAssembly()
		{
		    if (!saveGeneratedAssembly) return;
		    assemblyBuilder.Save(generatedAssemblyName + ".dll");
		    assemblyBuilder = null; // reset
		}

		/// <summary>
		/// Read the interface declaration and emit a method for each method declaration
		/// </summary>
		/// <param name="builder"></param>
		protected virtual void GenerateMethodImpl(TypeBuilder builder)
		{
			GenerateMethodImpl(builder, typeof (TInterface));
		}

		protected virtual void GenerateMethodImpl(TypeBuilder builder, Type currentType)
		{
			MethodInfo[] methods = currentType.GetMethods();
			foreach (MethodInfo method in methods)
			{
				Type[] parameterTypes = GetParameters(method.GetParameters()); // declare the method with the correct parameters
				MethodBuilder methodBuilder = builder.DefineMethod(method.Name, defaultMethodAttributes, method.ReturnType, parameterTypes);

				// Start building the method
				methodBuilder.CreateMethodBody(null, 0);
				ILGenerator iLGenerator = methodBuilder.GetILGenerator();

				GenerateMethodImpl(method, parameterTypes, iLGenerator);

				// declare that we override the interface method
				builder.DefineMethodOverride(methodBuilder, method);
			}

			Type[] inheritedInterfaces = currentType.GetInterfaces();
			foreach (Type inheritedInterface in inheritedInterfaces)
			{
				GenerateMethodImpl(builder, inheritedInterface);
			}
		}

		protected abstract void GenerateMethodImpl(MethodInfo method, Type[] parameterTypes, ILGenerator iLGenerator);

		protected bool IsVoidMethod(MethodInfo methodInfo)
		{
		    return methodInfo.ReturnType == typeof (void);
		}

	    protected MethodInfo GetMethodFromBaseClass(string methodName)
		{
			return baseClassType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
		}

		/// <summary>
		/// Simply generate a constructor:
		/// public MyClass(string configName)
		///		: base ( configName )
		/// {
		/// }
		/// </summary>
		/// <param name="builder"></param>
		protected virtual void GenerateConstructor(TypeBuilder builder)
		{
			// Define the constructor
			Type[] constructorParameters = new[] {typeof (string)};
			ConstructorBuilder constructorBuilder = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName, CallingConventions.Standard, constructorParameters);
			ILGenerator iLGenerator = constructorBuilder.GetILGenerator();

			iLGenerator.Emit(OpCodes.Ldarg_0); // this
			iLGenerator.Emit(OpCodes.Ldarg_1); // load the param

			// Call the base constructor
			ConstructorInfo originalConstructor = baseClassType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, constructorParameters, null);
			iLGenerator.Emit(OpCodes.Call, originalConstructor);

			iLGenerator.Emit(OpCodes.Ret);
		}

		#region Utils

		private static Type[] GetParameters(ParameterInfo[] declareParams)
		{
			List<Type> parameters = new List<Type>();
			foreach (ParameterInfo param in declareParams)
			{
				parameters.Add(param.ParameterType);
			}
			return parameters.ToArray();
		}

		#endregion

		#region Static Utils

		public Type GenerateType(string className)
		{
			generatedClassName = className;
			generatedAssemblyName = ASSEMBLY_NAME + generatedClassName;

			GenerateAssembly();

			Type type = moduleBuilder.GetType(generatedClassName);
			if (type != null)
			{
				// we already created this type ?
				return type;
			}

			// Generate a new type
			type = GenerateTypeImplementation();
			return type;
		}

		#endregion
	}
}