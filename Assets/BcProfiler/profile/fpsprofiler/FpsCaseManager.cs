using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FpsCaseManager{

	public enum FPS_CASE
	{
		FC_STAGE,	
		FC_BP,
		FC_STAGE_EFFECT,
		FC_UI_NOTE,
		FC_AVATAR_OTHER_TRANSPARENT,
		FC_AVATAR_OTHER_UNTRANSPARENT,

		FC_UI,
        FC_AVATAR_BP,
        FC_AVATAR_LINKSKINMESH,
        FC_AVATAR_PARTICLE,
        FC_AVATAR_BLEND,
        FC_AVATAR_NAMEPLATE,
        FC_AVATAR_OTHER,
        FC_SHEQU_BUILD,
        FC_SHEQU_TREE,
        FC_SHEQU_POOL,
        FC_SHEQU_OCEAN,
        FC_SHEQU_SKY,
        FC_SHEQU_TERRAIN,
        FC_SHEQU_OTHER,
        FC_NPC
	}

	protected FpsCase bpFpsCase = null;
	protected Dictionary<FPS_CASE, FpsCase> m_fpsCaseDic = null;
	public Dictionary<FPS_CASE, FpsCase> fpsCaseDic
	{
		get { return m_fpsCaseDic;}
	}
	public FpsCaseManager()
	{
        m_fpsCaseDic = new Dictionary<FPS_CASE, FpsCase>();
        //m_fpsCaseDic.Add(FPS_CASE.FC_STAGE, new FpsCaseStage());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_UI, new FpsCaseUI());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_UI_NOTE, new FpsCaseNote());
        //m_fpsCaseDic.Add(FPS_CASE.FC_UI, new FpsCaseUI());
        //m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_BP, new FpsCaseAvatarBP());
        //m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_PARTICLE, new FpsCaseAvatarParticle());
        //m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_LINKSKINMESH, new FpsCaseAvatarLinkSkinMesh());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_BLEND, new FpsCaseAvatarBlend());
        //m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_NAMEPLATE, new FpsCaseAvatarNamePlate());
        //m_fpsCaseDic.Add(FPS_CASE.FC_AVATAR_OTHER, new FpsCaseAvatarOther());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_BUILD, new FpsCaseBuild());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_TREE, new FpsCaseTree());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_POOL, new FpsCasePool());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_OCEAN, new FpsCaseOcean());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_SKY, new FpsCaseSky());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_TERRAIN, new FpsCaseTerrain());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_SHEQU_OTHER, new FpsCaseShequOther());
        ////m_fpsCaseDic.Add(FPS_CASE.FC_NPC, new FpsCaseNPC());
	}

	public void show(FPS_CASE fpsCase, bool bShow)
	{
		if (!fpsCaseDic.ContainsKey (fpsCase))
			return;
			
		fpsCaseDic [fpsCase].show (bShow);
	}

	public void showAll(bool bShow)
	{
		int iCount = fpsCaseDic.Count;
        var fpsCaseNode = fpsCaseDic.GetEnumerator();
        while (fpsCaseNode.MoveNext())
        {
            fpsCaseNode.Current.Value.show(bShow);
        }
	}
}
