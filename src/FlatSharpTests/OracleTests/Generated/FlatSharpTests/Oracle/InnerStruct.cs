// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatSharpTests.Oracle
{

using global::System;
using global::FlatBuffers;

public struct InnerStruct : IFlatbufferObject
{
  private Struct __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public InnerStruct __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int A { get { return __p.bb.GetInt(__p.bb_pos + 0); } }

  public static Offset<InnerStruct> CreateInnerStruct(FlatBufferBuilder builder, int A) {
    builder.Prep(4, 4);
    builder.PutInt(A);
    return new Offset<InnerStruct>(builder.Offset);
  }
};


}
