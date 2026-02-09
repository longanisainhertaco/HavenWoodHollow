# Troubleshooting Guide

Common issues and solutions for the Unity MCP Plugin.

## Connection Issues

### "Not connected to Unity"

**Symptoms:**
- Claude reports "Not connected to Unity"
- `unity_status` shows disconnected

**Solutions:**

1. **Verify Unity is running**
   - Make sure Unity Editor is open with your project

2. **Check Unity Console**
   - Look for `[MCP Bridge] Initialized on port 8090`
   - If missing, the plugin isn't loaded

3. **Restart Unity**
   - Close and reopen Unity
   - The plugin initializes on startup

4. **Check port availability**
   ```bash
   # macOS/Linux
   lsof -i :8090
   
   # Windows
   netstat -ano | findstr 8090
   ```
   If port is in use, change it in MCPBridge.cs

5. **Try manual connection**
   - Ask Claude: "Connect to Unity on port 8090"

### "Connection timeout"

**Symptoms:**
- Connection attempt times out after 10 seconds

**Solutions:**

1. **Check firewall**
   - Allow localhost connections on port 8090
   - The plugin only accepts local connections

2. **Verify plugin is loaded**
   - Check Unity Console for initialization message

3. **Check for exceptions**
   - Look for errors in Unity Console
   - MCPBridge might have crashed during startup

### Frequent disconnections

**Symptoms:**
- Connection drops repeatedly
- Works briefly then disconnects

**Solutions:**

1. **Unity is recompiling**
   - Normal during script changes
   - Server will auto-reconnect

2. **Reduce script changes**
   - Batch multiple script edits
   - Wait for compilation to finish

3. **Check Unity stability**
   - Unity might be crashing
   - Check Editor.log for errors

## Script Issues

### "Script not found" after creation

**Symptoms:**
- Created script but can't attach it
- Component type not recognized

**Solutions:**

1. **Wait for compilation**
   - Unity needs time to compile new scripts
   - Check `EditorApplication.isCompiling`

2. **Check for compile errors**
   ```
   Ask Claude: Check the Unity console for errors
   ```

3. **Verify file location**
   - Script must be in Assets folder
   - Check the path in tool response

### Compilation errors

**Symptoms:**
- Script created but has errors
- Unity Console shows red errors

**Solutions:**

1. **Read error details**
   ```
   Ask Claude: What errors are in the Unity console?
   ```

2. **Fix syntax issues**
   - Ask Claude to fix the specific error
   - Check line numbers in error messages

3. **Missing using statements**
   - Common issue with generated code
   - Ask Claude to add required namespaces

### Script changes not taking effect

**Symptoms:**
- Edited script but behavior unchanged
- Old version seems to be running

**Solutions:**

1. **Force refresh**
   ```
   Ask Claude: Refresh the asset database
   ```

2. **Check Play mode**
   - Changes don't apply during Play mode
   - Exit Play mode first

3. **Clear script cache**
   - Delete `Library/ScriptAssemblies` folder
   - Restart Unity

## GameObject Issues

### "GameObject not found"

**Symptoms:**
- Can't find object by name
- Modify operations fail

**Solutions:**

1. **Use exact name**
   - Names are case-sensitive
   - Check for typos

2. **Use full path**
   - For nested objects: "Parent/Child/Object"
   - Or use root-level names

3. **Check active state**
   - Inactive objects might not be found
   - Use `includeInactive: true`

4. **Verify scene loaded**
   - Object might be in different scene
   - Check current scene name

### Components not adding

**Symptoms:**
- AddComponent fails
- "Type not found" errors

**Solutions:**

1. **Use correct type name**
   - `Rigidbody` not `rigidbody`
   - `BoxCollider` not `Box Collider`

2. **Include namespace**
   - For custom scripts: `MyNamespace.MyComponent`
   - For UI: `UnityEngine.UI.Image`

3. **Check script compiled**
   - Custom components need successful compile
   - Wait for compilation

## Build Issues

### Build fails

**Symptoms:**
- `unity_build` reports failure
- Build errors in console

**Solutions:**

1. **Check build settings**
   ```
   Ask Claude: Show me the current build settings
   ```

2. **Verify scenes**
   - All scenes must be in build settings
   - Check for missing scene references

3. **Check platform support**
   - Platform module must be installed
   - Unity Hub > Installs > Add Modules

4. **Review console errors**
   - Build-specific errors shown
   - Fix before rebuilding

### Wrong platform

**Symptoms:**
- Building for wrong platform
- Platform-specific errors

**Solutions:**

1. **Switch platform first**
   ```
   Ask Claude: Switch to Windows build target
   ```

2. **Wait for switch**
   - Platform switch can take time
   - Assets need reimporting

## Performance Issues

### Slow operations

**Symptoms:**
- Commands take long time
- Unity becomes unresponsive

**Solutions:**

1. **Reduce hierarchy size**
   - Large hierarchies slow down queries
   - Use filters to limit results

2. **Batch operations**
   - Create multiple objects together
   - Reduce round-trips

3. **Avoid during compilation**
   - Wait for compilation to finish
   - Operations queue during compile

### Memory issues

**Symptoms:**
- Unity memory grows
- Editor becomes slow

**Solutions:**

1. **Close unused scenes**
   - Unload unnecessary scenes

2. **Clean up assets**
   - Remove unused imports
   - Delete test objects

3. **Restart Unity**
   - Sometimes necessary to clear memory

## Plugin Issues

### Plugin not loading

**Symptoms:**
- No initialization message in Console
- Unity loads but no MCP Bridge

**Solutions:**

1. **Check package location**
   - Must be in `Packages/` or `Assets/Editor/`
   - Verify folder structure

2. **Check assembly definition**
   - `UnityMCPBridge.Editor.asmdef` must exist
   - Editor platform only

3. **Check for errors**
   - Compilation errors prevent loading
   - Look for red errors in Console

4. **Reimport package**
   - Right-click > Reimport
   - Or delete and reinstall

### Wrong Unity version

**Symptoms:**
- API errors
- Missing methods

**Solutions:**

1. **Check requirements**
   - Minimum: Unity 2021.3
   - Recommended: Unity 2022.3+

2. **Update Unity**
   - Use Unity Hub to update

3. **Check deprecations**
   - Some APIs changed in newer versions
   - Plugin may need update

## Getting Help

### Collect Information

Before reporting issues, gather:

1. **Unity version**: Help > About Unity
2. **Plugin version**: Check package.json
3. **Error messages**: Copy from Console
4. **Steps to reproduce**: What you did

### Debug Mode

Enable verbose logging:

1. In MCPBridge.cs, add:
   ```csharp
   Debug.Log($"[MCP Bridge] Debug: {message}");
   ```

2. Check Console for detailed logs

### Report Issues

Open an issue with:
- Unity version
- OS (Windows/macOS/Linux)
- Full error message
- Steps to reproduce
- What you expected vs what happened
