using System;
using System.Reflection;
using System.Reflection.Emit;
using Meadow.Reflection.ObjectTree;

namespace Meadow.Reflection.Dynamics
{
    public class ModelBuilder
    {
        private readonly TypeBuilder _typeBuilder;


        public ModelBuilder(string typeName, string assemblyName = "DynamicModelsAssembly",
            string moduleName = "DynamicModelsModule")
        {
            var assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);

            _typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class);
        }


        public ModelBuilder AddProperty(string name, Type type)
        {
            AddProperty(_typeBuilder, name, type);

            return this;
        }

        private void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var propertyBuilder =
                typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new Type[] { });

            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var getter =
                typeBuilder.DefineMethod("Get" + propertyName + "Accessor",
                    MethodAttributes.Public,
                    propertyType,
                    Type.EmptyTypes);

            var setter =
                typeBuilder.DefineMethod("Set" + propertyName + "Accessor",
                    MethodAttributes.Public,
                    null,
                    new Type[] {propertyType});


            var getterIlGen = getter.GetILGenerator();
            var setterIlGen = setter.GetILGenerator();


            getterIlGen.Emit(OpCodes.Ldarg_0);
            getterIlGen.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIlGen.Emit(OpCodes.Ret);

            setterIlGen.Emit(OpCodes.Ldarg_0);
            setterIlGen.Emit(OpCodes.Ldarg_1);
            setterIlGen.Emit(OpCodes.Stfld, fieldBuilder);
            setterIlGen.Emit(OpCodes.Ret);


            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
        }


        public Type Build()
        {
            return _typeBuilder.CreateType();
        }


        public object BuildObject()
        {
            var type = Build();

            return new TypeAnalyzer().CreateObject(type);
        }
    }
}