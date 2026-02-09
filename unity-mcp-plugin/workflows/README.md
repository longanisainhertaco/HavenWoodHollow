# Unity Game Development Workflows

This directory contains pre-defined workflows for common game development tasks.
Each workflow guides Claude through a sequence of steps to accomplish complex goals.

## Available Workflows

### 1. New Project Setup (`new-project-setup.yaml`)
Complete project initialization with folder structure, settings, and basic setup.

### 2. Character Creation (`character-creation.yaml`)
Creates a player or NPC character with movement, animation, and basic AI.

### 3. Level Design (`level-design.yaml`)
Sets up a game level with environment, lighting, and gameplay elements.

### 4. UI System (`ui-system.yaml`)
Creates UI screens with menus, HUD, and navigation.

### 5. Build & Deploy (`build-deploy.yaml`)
Builds the project for various platforms with optimization.

## Workflow Structure

Each workflow YAML file follows this structure:

```yaml
name: workflow-name
description: What the workflow accomplishes
version: "1.0"

# Pre-conditions that must be met
prerequisites:
  - Connected to Unity
  - Scene is loaded

# Input parameters
parameters:
  - name: characterName
    type: string
    required: true
    description: Name for the character

# Workflow steps
steps:
  - name: step-name
    description: What this step does
    tool: unity_tool_name
    params:
      param1: "{{characterName}}"
    on_error: retry|skip|abort
    
  - name: conditional-step
    condition: "{{previousResult.success}}"
    description: Only runs if condition is true
    tool: another_tool
```

## Creating Custom Workflows

1. Create a new YAML file in this directory
2. Define parameters the workflow needs
3. List steps in execution order
4. Handle errors appropriately
5. Document the workflow in README

## Best Practices

- Keep workflows focused on one goal
- Use descriptive step names
- Include error handling
- Document all parameters
- Test workflows thoroughly
