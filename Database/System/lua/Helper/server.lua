print("重新加载server.lua")
function server_get_soul(物品编号)
    local configMgr = ConfigMgr:Instance()
    local 配置 = configMgr:GetItem("soul", 物品编号)
    return 配置
end

function engine_gc()
    local count = collectgarbage("count")
    -- 借鉴同源: 用增量步进GC(step)替代全量collect, 消除每5秒一次"停止世界"全量GC的停顿尖峰(偶发卡顿主因之一);
    -- Lua默认自动增量GC仍持续按分配回收内存, 此处只作轻量推进。如需偶尔彻底回收可周期性再调一次collect。
    collectgarbage("step", 200)
    local collect2 = collectgarbage("count")
    local collected = collect2 - count
    --print("本次收集垃圾数量:"..collected.."kb")
    return collect2
end
