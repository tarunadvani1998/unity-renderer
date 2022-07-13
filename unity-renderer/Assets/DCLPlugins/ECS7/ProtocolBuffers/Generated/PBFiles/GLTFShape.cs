// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: GLTFShape.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace DCL.ECSComponents {

  /// <summary>Holder for reflection information generated from GLTFShape.proto</summary>
  public static partial class GLTFShapeReflection {

    #region Descriptor
    /// <summary>File descriptor for GLTFShape.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GLTFShapeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9HTFRGU2hhcGUucHJvdG8SEGRlY2VudHJhbGFuZC5lY3MiYAoLUEJHTFRG",
            "U2hhcGUSFwoPd2l0aF9jb2xsaXNpb25zGAEgASgIEhoKEmlzX3BvaW50ZXJf",
            "YmxvY2tlchgCIAEoCBIPCgd2aXNpYmxlGAMgASgIEgsKA3NyYxgEIAEoCUIU",
            "qgIRRENMLkVDU0NvbXBvbmVudHNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::DCL.ECSComponents.PBGLTFShape), global::DCL.ECSComponents.PBGLTFShape.Parser, new[]{ "WithCollisions", "IsPointerBlocker", "Visible", "Src" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PBGLTFShape : pb::IMessage<PBGLTFShape>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<PBGLTFShape> _parser = new pb::MessageParser<PBGLTFShape>(() => new PBGLTFShape());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<PBGLTFShape> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::DCL.ECSComponents.GLTFShapeReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PBGLTFShape() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PBGLTFShape(PBGLTFShape other) : this() {
      withCollisions_ = other.withCollisions_;
      isPointerBlocker_ = other.isPointerBlocker_;
      visible_ = other.visible_;
      src_ = other.src_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PBGLTFShape Clone() {
      return new PBGLTFShape(this);
    }

    /// <summary>Field number for the "with_collisions" field.</summary>
    public const int WithCollisionsFieldNumber = 1;
    private bool withCollisions_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool WithCollisions {
      get { return withCollisions_; }
      set {
        withCollisions_ = value;
      }
    }

    /// <summary>Field number for the "is_pointer_blocker" field.</summary>
    public const int IsPointerBlockerFieldNumber = 2;
    private bool isPointerBlocker_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool IsPointerBlocker {
      get { return isPointerBlocker_; }
      set {
        isPointerBlocker_ = value;
      }
    }

    /// <summary>Field number for the "visible" field.</summary>
    public const int VisibleFieldNumber = 3;
    private bool visible_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Visible {
      get { return visible_; }
      set {
        visible_ = value;
      }
    }

    /// <summary>Field number for the "src" field.</summary>
    public const int SrcFieldNumber = 4;
    private string src_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Src {
      get { return src_; }
      set {
        src_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as PBGLTFShape);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(PBGLTFShape other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (WithCollisions != other.WithCollisions) return false;
      if (IsPointerBlocker != other.IsPointerBlocker) return false;
      if (Visible != other.Visible) return false;
      if (Src != other.Src) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (WithCollisions != false) hash ^= WithCollisions.GetHashCode();
      if (IsPointerBlocker != false) hash ^= IsPointerBlocker.GetHashCode();
      if (Visible != false) hash ^= Visible.GetHashCode();
      if (Src.Length != 0) hash ^= Src.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (WithCollisions != false) {
        output.WriteRawTag(8);
        output.WriteBool(WithCollisions);
      }
      if (IsPointerBlocker != false) {
        output.WriteRawTag(16);
        output.WriteBool(IsPointerBlocker);
      }
      if (Visible != false) {
        output.WriteRawTag(24);
        output.WriteBool(Visible);
      }
      if (Src.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Src);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (WithCollisions != false) {
        output.WriteRawTag(8);
        output.WriteBool(WithCollisions);
      }
      if (IsPointerBlocker != false) {
        output.WriteRawTag(16);
        output.WriteBool(IsPointerBlocker);
      }
      if (Visible != false) {
        output.WriteRawTag(24);
        output.WriteBool(Visible);
      }
      if (Src.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Src);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (WithCollisions != false) {
        size += 1 + 1;
      }
      if (IsPointerBlocker != false) {
        size += 1 + 1;
      }
      if (Visible != false) {
        size += 1 + 1;
      }
      if (Src.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Src);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(PBGLTFShape other) {
      if (other == null) {
        return;
      }
      if (other.WithCollisions != false) {
        WithCollisions = other.WithCollisions;
      }
      if (other.IsPointerBlocker != false) {
        IsPointerBlocker = other.IsPointerBlocker;
      }
      if (other.Visible != false) {
        Visible = other.Visible;
      }
      if (other.Src.Length != 0) {
        Src = other.Src;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            WithCollisions = input.ReadBool();
            break;
          }
          case 16: {
            IsPointerBlocker = input.ReadBool();
            break;
          }
          case 24: {
            Visible = input.ReadBool();
            break;
          }
          case 34: {
            Src = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            WithCollisions = input.ReadBool();
            break;
          }
          case 16: {
            IsPointerBlocker = input.ReadBool();
            break;
          }
          case 24: {
            Visible = input.ReadBool();
            break;
          }
          case 34: {
            Src = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
