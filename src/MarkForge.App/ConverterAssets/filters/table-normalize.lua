-- Phase 1 baseline filter for table pass-through.
-- This is intentionally minimal and establishes the Lua filter hook for future fidelity work.
function Table(tbl)
  return tbl
end
