// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::FlatBuffers;

public struct fid : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static fid GetRootAsfid(ByteBuffer _bb) { return GetRootAsfid(_bb, new fid()); }
  public static fid GetRootAsfid(ByteBuffer _bb, fid obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public fid __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public eFB_Type FType { get { int o = __p.__offset(4); return o != 0 ? (eFB_Type)__p.bb.GetInt(o + __p.bb_pos) : eFB_Type.Base; } }
  public int Id { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<fid> Createfid(FlatBufferBuilder builder,
      eFB_Type fType = eFB_Type.Base,
      int id = 0) {
    builder.StartObject(2);
    fid.AddId(builder, id);
    fid.AddFType(builder, fType);
    return fid.Endfid(builder);
  }

  public static void Startfid(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddFType(FlatBufferBuilder builder, eFB_Type fType) { builder.AddInt(0, (int)fType, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(1, id, 0); }
  public static Offset<fid> Endfid(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<fid>(o);
  }
};

