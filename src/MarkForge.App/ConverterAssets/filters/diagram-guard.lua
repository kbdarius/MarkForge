-- Phase 1 baseline filter for diagram code blocks.
-- Keeps Mermaid blocks explicit so the conversion pipeline can attach richer handling in Phase 2.
function CodeBlock(block)
  if block.classes:includes("mermaid") then
    return block
  end

  return nil
end
